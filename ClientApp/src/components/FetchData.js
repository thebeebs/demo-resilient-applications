import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { jokes: [], loading: true };
    this.populateJokeData = this.populateJokeData.bind(this);
  }

  componentDidMount() {
    this.populateJokeData();
  }

  static renderJokesTable(jokes) {
    return (
      
        
        <div>
          {jokes.map(joke =>
             <div>
               <div class="row">
                 <div class="col-12">&nbsp;
                 </div>
               </div>
               <div class="row">
                 <div class="col-12">
                <div class="w-100 border talk-bubble tri-right round left-top">
                  <div class="talktext text-center"><p>{joke.setUp}</p></div>
                </div>
                 </div>
               </div>
               <div class="row">
                 <div class="col-12">
                <div class="w-100 border talk-bubble tri-right round right-top">
                  <div class=" talktext text-center"><p>{joke.punchLine}</p></div>
                </div>
                 </div>
               </div>
               <div class="row">
                 <div class="offset-3 col-6">
                   <div class="alert alert-primary text-center" role="alert">
                     Source: {joke.source}
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
      ? <div class="mx-auto p-5 w-25"><p class="mx-auto w-100 text-light"><i className="fa fa-refresh fa-spin fa-3x fa-fw"/><em>Loading...</em></p></div>
      : FetchData.renderJokesTable(this.state.jokes);
    
    let button = this.state.loading
        ? <div class="mx-auto p-3 w-25">
          </div>
        : <div class="mx-auto p-3 w-25">
            <button className="btn btn-primary btn-lg mx-auto w-100" onClick={this.populateJokeData}>Tell Me Another</button>
          </div>

    return (
      <div>
        {contents}
        {button}
      </div>
    );
  }

  async populateJokeData() {
    this.setState({ loading: true });
    const response = await fetch('api/Joke');
    const data = await response.json();
    setTimeout(() => {
      this.setState({ jokes: data, loading: false });
    }, 500);
    
  }
  
}
