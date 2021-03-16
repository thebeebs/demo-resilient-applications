import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { forecasts: [], loading: true };
    this.populateWeatherData = this.populateWeatherData.bind(this);
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(forecasts) {
    return (
      
        
        <div>
          {forecasts.map(forecast =>
             <div> 
              <div class="w-100 border talk-bubble tri-right round left-top">
                <div class="talktext">
                  <p>{forecast.setUp}</p>
                </div>
              </div>
            <div class="w-100 border talk-bubble tri-right round right-top">
            <div class="talktext">
            <p>{forecast.punchLine}</p>
            </div>
            </div>
               </div>
          )}
        
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><i className="fa fa-refresh fa-spin fa-3x fa-fw"></i><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.forecasts);
    
    let button = this.state.loading
        ? <div class="mx-auto p-3 w-25">
          </div>
        : <div class="mx-auto p-3 w-25">
            <button className="btn btn-primary btn-lg mx-auto w-100" onClick={this.populateWeatherData}>Tell Me Another</button>
          </div>

    return (
      <div>
        {contents}
        {button}
      </div>
    );
  }

  async populateWeatherData() {
    this.setState({ loading: true });
    const response = await fetch('api/Joke');
    const data = await response.json();
    this.setState({ forecasts: data, loading: false });
  }
  
}
