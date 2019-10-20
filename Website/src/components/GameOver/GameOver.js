import React, { PureComponent } from "react";
import "./GameOver.scss";

class GameOver extends PureComponent {
  render() {
    const { points, vacuumedBy } = this.props;
    return (
      <div className="game-over">
        <div className="game-over--content">
          <h1>Game Over!</h1>

          <div className="game-over--statistics">
            <div>Points: {points}</div>
            <div>You were vacuumed by: {vacuumedBy}</div>

            <a href="/">Restart</a>
          </div>
        </div>
      </div>
    );
  }
}

export default GameOver;
