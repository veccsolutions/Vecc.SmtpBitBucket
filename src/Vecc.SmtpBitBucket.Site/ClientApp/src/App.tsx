import * as React from 'react';
import { Route, Switch } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';
import FetchSessions from './components/Sessions';
import FetchSessionDetails from './components/SessionDetails';
import './custom.css'

export default () => (
    <Layout>
        <Switch>
            <Route exact path='/' component={Home} />
            <Route path='/counter' component={Counter} />
            <Route path='/sessions/detail/:sessionId+' component={FetchSessionDetails} />
            <Route path='/sessions' component={FetchSessions} />
            <Route path='/fetch-data/:startDateIndex?' component={FetchData} />
        </Switch>
    </Layout>
);
