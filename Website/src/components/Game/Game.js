import React, { PureComponent } from "react";
import p5 from "p5";
import { HttpTransportType, HubConnectionBuilder } from "@aspnet/signalr";

import GUI from "../GUI";
import GameOver from "../GameOver";

var heightOfTheWorld = 7000;
var widthOfTheWorld = 7000;

var initialSizeOfVacuum = 32;

var zoom = 1;

function VacuumCleaner(
  sketch,
  size,
  xCoorinate,
  yCoordinate,
  onEat = () => { },
  onUpdate = () => { },
  data
) {
  this.r = size;
  this.pos = sketch.createVector(xCoorinate, yCoordinate);
  this.velocity = sketch.createVector(0, 0);

  this.update = function () {
    const xDist = sketch.mouseX - sketch.width / 2;
    const yDist = sketch.mouseY - sketch.height / 2;

    const vel = sketch.createVector(xDist, yDist);

    vel.setMag(10);

    // Detects boundaries of world and disables movement
    const canMoveX =
      (this.pos.x < widthOfTheWorld && vel.x > 0) ||
      (this.pos.x > -widthOfTheWorld && vel.x < 0);

    const canMoveY =
      (this.pos.y < heightOfTheWorld && vel.y > 0) ||
      (this.pos.y > -heightOfTheWorld && vel.y < 0);

    const speed =
      Math.abs(xDist) > 5 || Math.abs(yDist) > 5 ? this.velocity : 0;

    if (!canMoveX && speed) {
      speed.x = 0;
    }

    if (!canMoveY && speed) {
      speed.y = 0;
    }

    this.velocity.lerp(vel, 0.1);
    this.pos.add(speed);

    onUpdate();
  };

  this.eats = function (object) {
    var distance = sketch.dist(
      this.pos.x,
      this.pos.y,
      object.pos.x,
      object.pos.y
    );

    if (distance < this.r + object.r && this.r > object.r) {
      //Reduce Complexity
      var sum =
        sketch.PI * this.r * this.r + sketch.PI * object.r * object.r * 0.2;
      this.r = sketch.sqrt(sum / sketch.PI);
      onEat(object);
      return true;
    }

    return false;
  };

  this.show = function () {
    sketch.fill(255);
    sketch.textSize(34 * (this.r / 64));
    const sWidth = sketch.textWidth(data.username);
    sketch.text(
      data.username,
      this.pos.x - sWidth * 0.5,
      this.pos.y - this.r - 10 * (this.r / 64)
    );
    sketch.ellipse(this.pos.x, this.pos.y, this.r * 2, this.r * 2);
  };
}

function SpaceScrap(id, sketch, size, xCoorinate, yCoordinate, icon, data) {
  this.pos = sketch.createVector(xCoorinate, yCoordinate);
  this.r = size;
  this.id = id;

  this.data = data;

  this.show = function () {
    sketch.fill(255);
    sketch.image(icon, this.pos.x, this.pos.y, this.r * 2, this.r * 2);
  };
}

function GameObject(id, sketch, size, xCoorinate, yCoordinate, data) {
  this.pos = sketch.createVector(xCoorinate, yCoordinate);
  this.r = size;
  this.id = id;
  this.velocity = sketch.createVector(0, 0);
  this.data = data;
  this.show = function () {
    sketch.fill(data.color);
    sketch.textSize(34 * (this.r / 64));
    const sWidth = sketch.textWidth(data.username);
    sketch.text(
      data.username,
      this.pos.x - sWidth * 0.5,
      this.pos.y - this.r - 10 * (this.r / 64)
    );
    sketch.ellipse(this.pos.x, this.pos.y, this.r * 2, this.r * 2);
  };

  this.setPos = function (xCoorinate, yCoordinate) {
    this.pos = sketch.createVector(xCoorinate, yCoordinate);
  };

  this.update = function () { };

  this.resize = function (size) {
    this.r = size;
  };
}

class Game extends PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      points: 0,
      gameEnded: null,
      currentPosition: {
        x: 0,
        y: 0
      }
    };

    this.gameStarted = false;

    this.users = [];
    this.spaceScraps = [];
    this.satellites = [];
  }

  componentDidUpdate(prevProps) {
    if (prevProps.started !== this.props.started && this.props.started) {
      this.start();
    }
  }

  getRandomColor() {
    return this.sketch.color(
      this.sketch.random(255),
      this.sketch.random(255),
      this.sketch.random(255)
    );
  }

  onGameHubInitialize = objects => {
    if (objects) {
      objects.forEach(object => {
        if (object.GameObjectType === 2) {
          this.satellites.push(new SpaceScrap(
            null,
            this.sketch,
            object.Size,
            object.X,
            object.Y,
            this.satellite,
            {}));
        } else if (object.GameObjectType === 1) {
          this.users.push(
            new GameObject(
              object.Id,
              this.sketch,
              object.Size,
              object.X,
              object.Y,
              {
                username: object.Name,
                color: this.getRandomColor()
              }
            )
          );
        } else {
          this.spaceScraps.push(
            new SpaceScrap(
              object.Id,
              this.sketch,
              object.Size,
              object.X,
              object.Y,
              this.meteor,
              object
            )
          );
        }
      });
    }
  };

  handleHubConnected() {
    this.gameHub.send("Set_Username", this.props.data.username);
    this.gameHub.send("Join_Room", this.props.data.roomId);
    this.gameHub.send("Start_Game");

    this.gameHub.on("User_Digested", id => {
      let index = this.users.findIndex(x => x.id === id);
      if (index > -1) {
        this.users.splice(index, 1);
      }

      index = this.spaceScraps.findIndex(x => x.id === id);
      if (index > -1) {
        this.spaceScraps.splice(index, 1);
      }
    });

    this.gameHub.on("User_GameOver", username => {
      this.vacuumCleaner = null;
      this.setState({
        gameEnded: {
          vacuumedBy: username
        }
      });
    });

    this.gameHub.on("User_Disconnected", this.userDisconnected);

    this.gameHub.on("initialize", this.onGameHubInitialize);
    this.gameHub.on("User_Connected", this.newUserConnected);
    this.gameHub.on("User_Position_Changed", this.updatePlayerPosition);
    this.gameHub.on("User_Resize", this.userResize);
  }

  sendPosition = async () => {
    if (this.gameStarted && this.vacuumCleaner) {
      await this.gameHub.send("User_Moved", {
        position: { x: this.vacuumCleaner.pos.x, y: this.vacuumCleaner.pos.y }
      });
    }
  };

  newUserConnected = data => {
    this.users.push(
      new GameObject(data.Id, this.sketch, data.Size, data.X, data.Y, {
        username: data.Name,
        color: this.getRandomColor()
      })
    );
  };

  updatePlayerPosition = data => {
    const user = this.users.find(x => x.id === data.UserId);
    if (user) {
      user.setPos(data.Position.X, data.Position.Y);
    }
  };

  userDisconnected = userId => {
    for (var i = 0; i < this.users.length; i++) {
      if (userId === this.users[i].id) {
        this.users.splice(i, 1);
      }
    }
  };

  userResize = (userId, size) => {
    for (var i = 0; i < this.users.length; i++) {
      if (userId === this.users[i].id) {
        this.users[i].resize(size);
      }
    }
  };

  eatSoundEffect() {
    const msg = new SpeechSynthesisUtterance();
    const voices = window.speechSynthesis.getVoices();
    msg.voice = voices[10]; // Note: some voices don't support altering params
    msg.voiceURI = "native";
    msg.volume = 1; // 0 to 1
    msg.rate = 1; // 0.1 to 10
    msg.pitch = 2; //0 to 2
    msg.text = "nom";
    msg.lang = "en-US";
    speechSynthesis.speak(msg);
  }

  onEat = object => {
    this.eatSoundEffect();
    this.setState(prevProps => ({
      points: prevProps.points + 1
    }));

    this.gameHub.send("User_Digest", object.id);

    if (object.data) {
      window.log(write => {
        write
          .changeDelay(10)
          .typeString(
            `Space junk ${object.data.Name} vacumed at ${object.pos.x}, ${object.pos.y} <br />`
          )
          .start();
      });
    }
  };

  onVacuumCleanerUpdate = () => {
    this.sendPosition();
  };

  setup(sketch) {
    this.meteor = sketch.loadImage("meteor.svg");
    this.earth = sketch.loadImage("earth.png");
    this.satellite = sketch.loadImage("satellite.svg");

    sketch.createCanvas(window.innerWidth, window.innerHeight);
    sketch.frameRate(30);

    const min = -4000;
    const max = 4000;

    this.vacuumCleaner = new VacuumCleaner(
      sketch,
      initialSizeOfVacuum,
      Math.floor(Math.random() * (max - min + 1)) + min,
      Math.floor(Math.random() * (max - min + 1)) + min,
      this.onEat,
      this.onVacuumCleanerUpdate,
      this.props.data
    );
  }

  drawEarth() {
    const r = 1000;
    this.sketch.createVector(0, 0);
    this.sketch.image(this.earth, -500, -500, r, r);
  }

  drawSetalites() {
    for (let i = this.satellites.length - 1; i >= 0; i--) {
      if (this.satellites[i]) {
        this.satellites[i].show();
      }
    }
  }

  draw(sketch) {
    sketch.background(0);
    sketch.translate(sketch.width / 2, sketch.height / 2);

    if (this.vacuumCleaner) {
      const newZoom = initialSizeOfVacuum / (this.vacuumCleaner.r * 0.5);

      zoom = sketch.lerp(zoom, newZoom, 0.1);
      sketch.scale(zoom);

      this.setState({
        currentPosition: {
          x: this.vacuumCleaner.pos.x,
          y: this.vacuumCleaner.pos.y
        }
      });

      sketch.translate(-this.vacuumCleaner.pos.x, -this.vacuumCleaner.pos.y);

      this.vacuumCleaner.show();
      this.vacuumCleaner.update();

      this.drawEarth();

      this.drowSpaceScrap();

      this.drawSetalites();
    }

    // this.users.forEach(user => {
    //   user.show();
    //   user.update();
    // });
  }

  drowSpaceScrap() {
    for (let i = this.spaceScraps.length - 1; i >= 0; i--) {
      if (this.vacuumCleaner.eats(this.spaceScraps[i])) {
        this.spaceScraps.splice(i, 1);
      }
      if (this.spaceScraps[i]) {
        this.spaceScraps[i].show();
      }
    }
    for (let i = this.users.length - 1; i >= 0; i--) {
      if (this.vacuumCleaner.eats(this.users[i])) {
        this.users.splice(i, 1);
      }
      if (this.users[i]) {
        this.users[i].show();
      }
    }
  }

  connectToGameHub() {
    this.gameHub = new HubConnectionBuilder()
      .withUrl("https://spaceappgame.azurewebsites.net/gamehub", {
        transport: HttpTransportType.WebSockets
      })
      .build();

    this.gameHub.start().then(() => {
      this.gameStarted = true;
      this.handleHubConnected();
    });
  }

  start() {
    this.sk = sketch => {
      this.sketch = sketch;

      sketch.setup = () => {
        this.connectToGameHub();
        this.setup(sketch);
      };
      sketch.draw = () => {
        this.draw(sketch);
      };
    };

    this.p5 = new p5(this.sk);
  }

  render() {
    return (
      <>
        <GUI
          points={this.state.points}
          x={this.state.currentPosition.x.toFixed(6)}
          y={this.state.currentPosition.y.toFixed(6)}
        />

        {this.state.gameEnded && (
          <GameOver
            vacuumedBy={this.state.gameEnded.vacuumedBy}
            points={this.state.points}
          />
        )}
      </>
    );
  }
}

export default Game;
