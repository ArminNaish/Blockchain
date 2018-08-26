import json
import argparse

from textwrap import dedent
from flask import Flask, jsonify, request
from blockchain import Blockchain
from uuid import uuid4

# Instantiate our Node
app = Flask(__name__)

# Generate a globally unique address for this node
node_identifier = str(uuid4()).replace('-', '')

# Instantiate the Blockchain
blockchain = Blockchain()

@app.route('/mine', methods=['GET'])
def mine():
    """
    Our mining endpoint is where the magic happens, it has to do three things:
    1. Calculate the proof of work
    2. Reward the miner (us) by adding a transaction granting us 1 coin
    3. Forge the new block by adding it to the chain

    GET /mine
    """

    # We run the proof of work algorithm to get the next proof...
    last_block = blockchain.last_block
    last_proof = last_block['proof']
    proof = blockchain.proof_of_work(last_proof)

    # We must receive a reward for finding the proof.
    # The sender is "0" to signify that this node has mined a new coin.
    blockchain.new_transaction(
        sender="0",
        recipient=node_identifier,
        amount=1,
    )

    # Forge the new block by adding it to the chain
    previous_hash = blockchain.hash(last_block)
    block = blockchain.new_block(proof, previous_hash)

    response = {
        'message': "New block forged",
        'index': block['index'],
        'transactions': block['transactions'],
        'proof': block['proof'],
        'previous_hash': block['previous_hash'],
    }
    return jsonify(response), 200 # ok
  
@app.route('/transactions/new', methods=['POST'])
def new_transaction():
    """
    Add a new transaction to go into the next mined block

    POST /transactions/new
    {
        "sender": "my address",
        "recipient": "someone else's address",
        "amount": 5
    }
    """

    values = request.get_json()

    # Check that the required fields are in the POST'ed data
    required = ['sender', 'recipient', 'amount']
    if not all(k in values for k in required):
        return 'Missing values', 400 # bad request

    # Create a new transaction
    index = blockchain.new_transaction(values['sender'], values['recipient'], values['amount'])

    response = {'message': f'Transaction will be added to block {index}'}
    return jsonify(response), 201 # created

@app.route('/chain', methods=['GET'])
def full_chain():
    """
    GET /chain
    """

    response = {
        'chain': blockchain.chain,
        'length': len(blockchain.chain),
    }
    return jsonify(response), 200

@app.route('/nodes/register', methods=['POST'])
def register_nodes():
    """
    Register a list of new nodes

    POST /nodes/register
    {
        "nodes": ["http://127.0.0.1:5001"]
    }
    """

    values = request.get_json()

    nodes = values.get('nodes')
    if nodes is None:
        return "Error: Please supply a valid list of nodes", 400

    for node in nodes:
        blockchain.register_node(node)

    response = {
        'message': 'New nodes have been added',
        'total_nodes': list(blockchain.nodes),
    }
    return jsonify(response), 201 # created

@app.route('/nodes/resolve', methods=['GET'])
def consensus():
    """
    Resolve conflicts

    GET /nodes/resolve
    """

    replaced = blockchain.resolve_conflicts()

    if replaced:
        response = {
            'message': 'Our chain was replaced',
            'new_chain': blockchain.chain
        }
    else:
        response = {
            'message': 'Our chain is authoritative',
            'chain': blockchain.chain
        }

    return jsonify(response), 200 #ok

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument(
        '--port', type=int, choices=range(1024,49151),
        default=5000, help='the port used by the server 1024..49151')
    args = parser.parse_args()
    app.run(host='0.0.0.0', port=args.port)