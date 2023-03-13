import logo from './logo.svg';
import './App.css';
import React, { Component } from 'react';

class App extends Component {
    constructor() {
        super();
        this.state = {
            number: Number
        }
    }

    getNumber = async () => {
        var response = await (await fetch(
            'https://localhost:5001/get', {
            method: 'get'
        })).text();

        this.setState({
            number: response
        });
    }

    render() {
        return (
            <div className="App">
                <header className="App-header">
                    <img src={logo} className="App-logo" alt="logo" />
                    <button onClick={this.getNumber}>Get number</button>
                    <h6>Response - {this.state.number}</h6>
                </header>
            </div>
        );
    }
}

export default App;