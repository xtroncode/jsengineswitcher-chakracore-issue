import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import ReactDOMServer from 'react-dom/server';


global.React = React;
global.ReactDOM = ReactDOM;
global.ReactDOMServer = ReactDOMServer;



class ServerApp extends Component {
    render() {
      return (
        <>{this.props.a.b}</>)
    }
}

global.RootComponent = ServerApp;
