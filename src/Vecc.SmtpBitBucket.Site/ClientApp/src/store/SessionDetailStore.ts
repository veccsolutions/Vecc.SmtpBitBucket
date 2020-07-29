import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';
import * as Models from './models';
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface SessionDetailState {
    isLoading: boolean;
    sessionId?: string;
    sessionDetail?: Models.SessionDetail;
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
type KnownAction = RequestSessionDetailAction | ReceiveSessionDetailAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestSessionDetail: (sessionId: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const appState = getState();

        if (appState && appState.sessionDetails && sessionId !== appState.sessionDetails.sessionId) {
            const encodedSessionId = encodeURIComponent(sessionId);
            fetch(`http://localhost:5000/api/v1/sessions/details?id=${encodedSessionId}`)
                .then(response => {
                    return response.json() as Promise<Models.SessionDetail>;
                })
                .then(data => {
                    dispatch({ type: 'RECEIVE_SESSION_DETAIL', sessionId: sessionId, sessionDetail: data });
                })
                .catch(reason => {
                    console.log("Error reading data:");
                    console.log(reason);
                });

                dispatch({ type: 'REQUEST_SESSION_DETAIL', sessionId: sessionId });
            }
        }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SessionDetailState = {
    isLoading: false
};

export const reducer: Reducer<SessionDetailState> = (state: SessionDetailState | undefined, incomingAction: Action): SessionDetailState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_SESSION_DETAIL':
            console.log("Action");
            console.log(action);
            console.log("Action done");
            return {
                isLoading: true,
                sessionId: action.sessionId,
                sessionDetail: state.sessionDetail
            };
        case 'RECEIVE_SESSION_DETAIL':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            if (action.sessionId === state.sessionId) {
                console.log("Session id matched");
                return {
                    isLoading: false,
                    sessionId: action.sessionId,
                    sessionDetail: action.sessionDetail
                };
            }
            console.log("WTF?");
            console.log(action.sessionId);
            console.log(state);
            break;
    }

    return state;
};
