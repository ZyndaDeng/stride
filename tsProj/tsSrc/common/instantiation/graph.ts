export class Node<T> {

	readonly data: T;
	readonly incoming = new Map<string, Node<T>>();
	readonly outgoing = new Map<string, Node<T>>();

	constructor(data: T) {
		this.data = data;
	}
}

export class Graph<T> {

	private readonly _nodes = new Map<string, Node<T>>();

	constructor(private readonly _hashFn: (element: T) => string) {
		// empty
	}

	roots(): Node<T>[] {
		const ret: Node<T>[] = [];
		for (let node of this._nodes.values()) {
			if (node.outgoing.size === 0) {
				ret.push(node);
			}
		}
		return ret;
	}

	insertEdge(from: T, to: T): void {
		const fromNode = this.lookupOrInsertNode(from);
		const toNode = this.lookupOrInsertNode(to);

		fromNode.outgoing.set(this._hashFn(to), toNode);
		toNode.incoming.set(this._hashFn(from), fromNode);
	}

	removeNode(data: T): void {
		const key = this._hashFn(data);
		this._nodes.delete(key);
		for (let node of this._nodes.values()) {
			node.outgoing.delete(key);
			node.incoming.delete(key);
		}
	}

	lookupOrInsertNode(data: T): Node<T> {
		const key = this._hashFn(data);
		let node = this._nodes.get(key);

		if (!node) {
			node = new Node(data);
			this._nodes.set(key, node);
		}

		return node;
	}

	lookup(data: T): Node<T> | undefined {
		return this._nodes.get(this._hashFn(data));
	}

	isEmpty(): boolean {
		return this._nodes.size === 0;
	}

	toString(): string {
		let data: string[] = [];
		for (let item of this._nodes) {
			const key=item[0];
			const value=item[1];
			data.push(`${key}, (incoming)[${[...value.incoming.keys()].join(', ')}], (outgoing)[${[...value.outgoing.keys()].join(',')}]`);

		}
		return data.join('\n');
	}
}