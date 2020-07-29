import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import { ApplicationState } from '../store';
import * as SessionDetailStore from '../store/SessionDetailStore';
import { Session } from 'inspector';
import * as Models from '../store/models';

// At runtime, Redux will merge together...
type SessionDetailProps =
  SessionDetailStore.SessionDetailState // ... state we've requested from the Redux store
  & typeof SessionDetailStore.actionCreators // ... plus action creators we've requested
  & RouteComponentProps<{ sessionId: string }>; // ... plus incoming routing parameters


class FetchSessionDetails extends React.PureComponent<SessionDetailProps> {
  // This method is called when the component is first added to the document
  public componentDidMount() {
    console.log("mounted");
    this.ensureDataFetched();
  }

  // This method is called when the route parameters change
  public componentDidUpdate() {
    console.log("updated");
    this.ensureDataFetched();
  }

  public render() {
    return (
      <React.Fragment>
        <h1 id="tabelLabel">Session Details</h1>
        {this.renderSectionsArea()}
      </React.Fragment>
    );
  }

  private ensureDataFetched() {
    const sessionId = this.props.match.params.sessionId;
    this.props.requestSessionDetail(sessionId);
    console.log("data requested");
  }

  private renderSectionsArea() {
    console.log(this.props);
    if (this.props.sessionDetail === undefined) {
      return (<div>Loading</div>);
    }
    var session = this.props.sessionDetail;

    var sessionChatter = "";
    if (session.sessionChatter) {
      session.sessionChatter.forEach(chatter => {
        var direction = ">";
        if (chatter.direction === 1) {
          direction = "<";
        }
        sessionChatter += `${chatter.timeStamp} -- ${direction} ${chatter.data}\r\n`;
      });
    }

    return (
      <div className="container">
        <div className="row">
          <h2 className="col-12">
            Metadata
          </h2>
          <div className="col-2">
            Remote Ip:
          </div>
          <div className="col-10">
            {session.remoteIp}
          </div>
          <div className="col-2">
            Start:
          </div>
          <div className="col-10">
            {session.sessionStartTime}
          </div>
          <div className="col-2">
            End:
          </div>
          <div className="col-10">
            {session.sessionEndTime}
          </div>
          <div className="col-2">
            Id:
          </div>
          <div className="col-10">
            {session.id}
          </div>
        </div>
        <div className="row">
          <h2 className="col-12">
            Messages
          </h2>
          {/* todo: message summaries */}
        </div>
        <div className="row">
          <h2 className="col-sm-12">
            Raw Session Chatter
          </h2>
          <div className="col-sm-12">
            <pre className="code">
              {sessionChatter}
            </pre>
          </div>
        </div>
      </div>);
  }
}

export default connect(
  (state: ApplicationState) => state.sessionDetails, // Selects which state properties are merged into the component's props
  SessionDetailStore.actionCreators // Selects which action creators are merged into the component's props
)(FetchSessionDetails as any);
