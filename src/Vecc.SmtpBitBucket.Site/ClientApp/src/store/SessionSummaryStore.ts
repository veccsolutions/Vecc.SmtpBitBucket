import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import * as Models from './models';
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface SessionSummaryState {
    isLoading: boolean;
    startDateIndex?: number;
    sessions?: Models.SessionSummaries;
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
            fetch(`http://localhost:5000/api/v1/sessions/summary`)
                .then(response => response.json() as Promise<Models.SessionSummaries>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_SESSIONS', startDateIndex: startDateIndex, sessions: data });
                });

            dispatch({ type: 'REQUEST_SESSIONS', startDateIndex: startDateIndex });
        }
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SessionSummaryState = {
    isLoading: false
};

export const reducer: Reducer<SessionSummaryState> = (state: SessionSummaryState | undefined, incomingAction: Action): SessionSummaryState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_SESSIONS':
            return {
                isLoading: true,
                startDateIndex: action.startDateIndex,
                sessions: state.sessions
            };
        case 'RECEIVE_SESSIONS':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            if (action.startDateIndex === state.startDateIndex) {
                return {
                    isLoading: false,
                    startDateIndex: action.startDateIndex,
                    sessions: action.sessions,
                };
            }
            break;
    }

    return state;
};
