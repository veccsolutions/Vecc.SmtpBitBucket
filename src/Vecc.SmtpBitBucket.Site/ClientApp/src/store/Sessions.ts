import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface SessionsState {
    isLoading: boolean;
    startDateIndex?: number;
    sessions: SessionSummary;
}

export interface SessionSummary{
    sessions: Session[];
}

export interface Session {
    date: string;
    sessionId: number;
    sessionStartTime: Date;
    sessionEndTime: Date;
    remoteIp: string;
    username: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestSessionsAction {
    type: 'REQUEST_SESSIONS';
    startDateIndex: number;
}

interface ReceiveSessionsAction {
    type: 'RECEIVE_SESSIONS';
    startDateIndex: number;
    sessions: SessionSummary;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestSessionsAction | ReceiveSessionsAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestSessions: (startDateIndex: number): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState && appState.sessions && startDateIndex !== appState.sessions.startDateIndex) {
            fetch(`http://localhost:5000/api/v1/sessions`)
                .then(response => response.json() as Promise<SessionSummary>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_SESSIONS', startDateIndex: startDateIndex, sessions: data });
                });

            dispatch({ type: 'REQUEST_SESSIONS', startDateIndex: startDateIndex });
        }
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SessionsState = { sessions: {sessions:[]}, isLoading: false };

export const reducer: Reducer<SessionsState> = (state: SessionsState | undefined, incomingAction: Action): SessionsState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_SESSIONS':
            return {
                startDateIndex: action.startDateIndex,
                sessions: state.sessions,
                isLoading: true
            };
        case 'RECEIVE_SESSIONS':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            if (action.startDateIndex === state.startDateIndex) {
                return {
                    startDateIndex: action.startDateIndex,
                    sessions: action.sessions,
                    isLoading: false
                };
            }
            break;
    }

    return state;
};
