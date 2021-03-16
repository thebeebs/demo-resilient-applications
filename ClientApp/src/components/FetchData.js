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
               <div class="row">
                 <div class="col-12">&nbsp;
                 </div>
               </div>
               <div class="row">
                 <div class="col-12">
                <div class="w-100 border talk-bubble tri-right round left-top">
                  <div class="talktext text-center"><p>{forecast.setUp}</p></div>
                </div>
                 </div>
               </div>
               <div class="row">
                 <div class="col-12">
                <div class="w-100 border talk-bubble tri-right round right-top">
                  <div class=" talktext text-center"><p>{forecast.punchLine}</p></div>
                </div>
                 </div>
               </div>
               <div class="row">
                 <div class="offset-3 col-6">
                   <div class="alert alert-primary text-center" role="alert">
                     Source: {forecast.source}
                   </div>
               </div>
                 
               </div>
                
            </div>
          )}
        
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <div class="mx-auto p-5 w-25"><p class="mx-auto w-100 text-light"><i className="fa fa-refresh fa-spin fa-3x fa-fw"></i><em>Loading...</em></p></div>
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
    setTimeout(() => {
      this.setState({ forecasts: data, loading: false });
    }, 500);
    
  }
  
}
