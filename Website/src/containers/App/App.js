import React, { PureComponent } from "react";
import axios from "axios";
import Space from "../../components/Space";
import Start from "../../components/Start";
import Game from "../../components/Game";
import "./App.scss";

class App extends PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      data: null,
      started: false
    };
  }

  onSubmit = async values => {
    if (values.roomId === "new-room") {
      axios
        .post("https://spaceappgame.azurewebsites.net/api/Rooms/CreateRoom")
        .then(({ data: room }) => {
          this.setState({
            data: {
              ...values,
              roomId: room.roomId
            },
            started: true
          });
        });
    } else {
      this.setState({
        data: values,
        started: true
      });
    }
  };

  render() {
    return (
      <>
        <Space />
        <Start onSubmit={this.onSubmit} />
        <Game started={this.state.started} data={this.state.data} />
      </>
    );
  }
}

export default App;
