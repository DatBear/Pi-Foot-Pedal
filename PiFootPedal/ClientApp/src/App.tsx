import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import BaseComponent from './components/BaseComponent';
import './custom.css'
import Setup from './components/Setup';


type AppProps = {
};

type AppState = {
  isDebug: boolean,
};

export default class App extends BaseComponent<AppProps, AppState> {
  static displayName = App.name;

  constructor(props: AppProps){
    super(props);
    this.state = {
      isDebug: window.location.href.indexOf('localhost') > -1
    };
  }

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/setup'>
          <Setup isDebug={this.state.isDebug} />
        </Route>
      </Layout>
    );
  }
}