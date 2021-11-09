
export function illegalState(name?: string): Error {
	if (name) {
		return new Error(`Illegal state: ${name}`);
	} else {
		return new Error('Illegal state');
	}
}

const canceledName = 'Canceled';

/**
 * Returns an error that signals cancellation.
 */
 export function canceled(): Error {
	const error = new Error(canceledName);
	error.name = error.message;
	return error;
}