import React, { PureComponent } from "react";
import cx from "classnames";
import { Form, Field, Formik } from "formik";
import axios from "axios";
import * as Yup from "yup";
import "./Start.scss";

class Start extends PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      visible: true,
      rooms: []
    };

    this.validationSchema = Yup.object().shape({
      username: Yup.string().required(),
      roomId: Yup.string().required()
    });
  }

  componentDidMount() {
    window.addEventListener("keypress", this.handleKeyPress);

    axios
      .get("https://spaceappgame.azurewebsites.net/api/Rooms/GetRooms")
      .then(({ data: rooms }) => {
        this.setState({
          rooms
        });
      });

    setTimeout(() => {
      this.readText();
    }, 1000);
  }

  componentWillUnmount() {
    window.removeEventListener("keypress", this.handleKeyPress);
  }

  handleKeyPress = event => {
    if (event.keyCode === 13) {
      this.callSubmit().then(() => {
        if (!Object.keys(this.errors).length) {
          window.log(write => {
            write.typeString("").start();
          });

          this.setState({
            visible: false
          });

          window.removeEventListener("keypress", this.handleKeyPress);
        }
      });
    }
  };

  readText() {
    const msg = new SpeechSynthesisUtterance();
    const voices = window.speechSynthesis.getVoices();
    msg.voice = voices[10]; // Note: some voices don't support altering params
    msg.voiceURI = "native";
    msg.volume = 1; // 0 to 1
    msg.rate = 1; // 0.1 to 10
    msg.pitch = 2; //0 to 2
    msg.text = "Space Crap!";
    msg.lang = "en-US";

    speechSynthesis.speak(msg);

    const msg2 = new SpeechSynthesisUtterance();
    msg2.text = "Press enter to begin";
    msg2.rate = 1.25; // 0.1 to 10
    
    speechSynthesis.speak(msg2);
  }

  render() {
    const { visible, rooms } = this.state;

    if (!visible) {
      return null;
    }

    return (
      <Formik
        initialValues={{
          username: "",
          roomId: ""
        }}
        validationSchema={this.validationSchema}
        onSubmit={this.props.onSubmit}
      >
        {({ errors, touched, submitForm }) => {
          this.callSubmit = submitForm;
          this.errors = errors;

          return (
            <Form>
              <div className="start">
                <h1>Space Crap!</h1>
                <h2>Press enter to begin</h2>

                <div className="start--select-room">
                  <Field
                    component="select"
                    name="roomId"
                    className={cx({
                      error: errors.roomId && touched.roomId
                    })}
                  >
                    <option>Select room</option>
                    <option value="new-room">New room</option>
                    {rooms.map(room => (
                      <option key={room.roomId} value={room.roomId}>
                        {room.roomName ? room.roomName : room.roomId}
                      </option>
                    ))}
                  </Field>
                </div>

                <div className="start--username">
                  <Field
                    type="text"
                    name="username"
                    className={cx({
                      error: errors.username && touched.username
                    })}
                    placeholder="Enter space cadet username"
                  />
                </div>
              </div>
            </Form>
          );
        }}
      </Formik>
    );
  }
}

export default Start;
