import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import { ApplicationState } from '../store';
import * as SessionsStore from '../store/Sessions';
import { Session } from 'inspector';
import * as Models from '../store/models';

// At runtime, Redux will merge together...
type SessionProps =
  SessionsStore.SessionsState // ... state we've requested from the Redux store
  & typeof SessionsStore.actionCreators // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


class FetchSession extends React.PureComponent<SessionProps> {
  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.ensureDataFetched();
  }

  // This method is called when the route parameters change
  public componentDidUpdate() {
    this.ensureDataFetched();
  }

  public render() {
    return (
      <React.Fragment>
        <h1 id="tabelLabel">Sessions</h1>
        {this.renderSectionsArea()}
        {this.renderPagination()}
      </React.Fragment>
    );
  }

  private ensureDataFetched() {
    const startDateIndex = parseInt(this.props.match.params.startDateIndex, 10) || 0;
    this.props.requestSessions(startDateIndex);
  }

  private renderSectionsArea() {

    if (this.props.sessions === undefined) {
      return (<div>Loading</div>);
    }
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Start Time</th>
            <th>End Time</th>
            <th>Username</th>
            <th>Remote IP</th>
            <th>Session ID</th>
          </tr>
        </thead>
        <tbody>
          {
            this.props.sessions.sessions.map((session: Models.SessionSummary) =>
              <tr key={session.sessionId} data-id={session.sessionId}>
                <td>{session.sessionStartTime}</td>
                <td>{session.sessionEndTime}</td>
                <td>{session.username}</td>
                <td>{session.remoteIp}</td>
                <td>{session.id}</td>
              </tr>
            )
          }
        </tbody>
      </table>);
  }

  private renderPagination() {
    const prevStartDateIndex = (this.props.startDateIndex || 0) - 5;
    const nextStartDateIndex = (this.props.startDateIndex || 0) + 5;

    return (
      <div className="d-flex justify-content-between">
        <Link className='btn btn-outline-secondary btn-sm' to={`/sessions/${prevStartDateIndex}`}>Previous</Link>
        {this.props.isLoading && <span>Loading...</span>}
        <Link className='btn btn-outline-secondary btn-sm' to={`/sessions/${nextStartDateIndex}`}>Next</Link>
      </div>
    );
  }
}

export default connect(
  (state: ApplicationState) => state.sessions, // Selects which state properties are merged into the component's props
  SessionsStore.actionCreators // Selects which action creators are merged into the component's props
)(FetchSession as any);
