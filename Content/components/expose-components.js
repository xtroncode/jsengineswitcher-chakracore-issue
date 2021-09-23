import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import ReactDOMServer from 'react-dom/server';


global.React = React;
global.ReactDOM = ReactDOM;
global.ReactDOMServer = ReactDOMServer;



class ServerApp extends Component {
    render() {
      const dateTime = new Date();
      const formattedTime = dateTime.toLocaleString("en-US", {
        hour: "numeric",
        minute: "numeric",
        hour12: true,
      });
      return (
        <div>{formattedTime}</div>
      )
    }
}

global.RootComponent = ServerApp;
