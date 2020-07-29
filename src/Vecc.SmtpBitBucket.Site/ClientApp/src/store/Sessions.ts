import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';
import * as Models from './models';
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface SessionsState {
    deatilIsLoading: boolean;
    isLoading: boolean;
    startDateIndex?: number;
    sessions?: Models.SessionSummaries;
    sessionId?: string;
    sessionDetail?: Models.SessionDetail;
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
    sessions: Models.SessionSummaries;
}

interface RequestSessionDetailAction {
    type: 'REQUEST_SESSION_DETAIL';
    sessionId: string;
}

interface ReceiveSessionDetailAction {
    type: 'RECEIVE_SESSION_DETAIL';
    sessionDetail: Models.SessionDetail;
    sessionId: string;
}
// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestSessionsAction | ReceiveSessionsAction | RequestSessionDetailAction | ReceiveSessionDetailAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestSessions: (startDateIndex: number): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState && appState.sessions && startDateIndex !== appState.sessions.startDateIndex) {
            fetch(`http://localhost:5000/api/v1/sessions/summary`)
                .then(response => response.json() as Promise<Models.SessionSummaries>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_SESSIONS', startDateIndex: startDateIndex, sessions: data });
                });

            dispatch({ type: 'REQUEST_SESSIONS', startDateIndex: startDateIndex });
        }
    },
    requestSessionDetail: (sessionId: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const appState = getState();
        fetch(`http://localhost:5000/api/v1/sessions/details?id=${sessionId}`)
            .then(response => response.json() as Promise<Models.SessionDetail>)
            .then(data => {
                dispatch({ type: 'RECEIVE_SESSION_DETAIL', sessionId: sessionId, sessionDetail: data });
            });

        dispatch({ type: 'REQUEST_SESSION_DETAIL', sessionId: sessionId });
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SessionsState = {
    deatilIsLoading: false,
    isLoading: false
};

export const reducer: Reducer<SessionsState> = (state: SessionsState | undefined, incomingAction: Action): SessionsState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_SESSIONS':
            return {
                deatilIsLoading: false,
                isLoading: true,
                startDateIndex: action.startDateIndex,
                sessions: state.sessions
            };
        case 'RECEIVE_SESSIONS':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            if (action.startDateIndex === state.startDateIndex) {
                return {
                    deatilIsLoading: false,
                    isLoading: false,
                    startDateIndex: action.startDateIndex,
                    sessions: action.sessions,
                };
            }
            break;
        case 'REQUEST_SESSION_DETAIL':
            return {
                deatilIsLoading: true,
                isLoading: false,
                sessionId: action.sessionId,
                sessionDetail: state.sessionDetail
            };
        case 'RECEIVE_SESSION_DETAIL':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            if (action.sessionId === state.sessionId) {
                return {
                    deatilIsLoading: false,
                    isLoading: false,
                    sessionId: action.sessionId,
                    sessionDetail: action.sessionDetail
                };
            }
            break;
    }

    return state;
};
