import React, { PureComponent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faInfoCircle, faTimes } from "@fortawesome/free-solid-svg-icons";
import Typewriter from "typewriter-effect/dist/core";
import GraphemeSplitter from "grapheme-splitter";
import cx from "classnames";
import "./GUI.scss";

class GUI extends PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      modal: false
    };
  }

  componentDidMount() {
    const stringSplitter = string => {
      const splitter = new GraphemeSplitter();
      return splitter.splitGraphemes(string);
    };

    const typewriter = new Typewriter(".gui--console", {
      delay: 75,
      stringSplitter
    });

    window.log = write => {
      write(typewriter);
    };

    this.introText();
  }

  introText() {
    window.log(write => {
      write
        .pauseFor(1500)
        .changeDelay(10)
        .typeString("Initializing...<br />")
        .pauseFor(2000)
        .typeString("Booting space...<br />")
        .pauseFor(300)
        .typeString("Polishing blackholes...<br />")
        .pauseFor(3000)
        .typeString("...<br />")
        .pauseFor(2000)
        .typeString(
          '<br /><span style="color: red;">... Exception ...</span><br /><br />'
        )
        .pauseFor(2000)
        .typeString("Swapping engines...<br />")
        .pauseFor(1000)
        .typeString(
          '<span style="color: green;">Ready for cleaning!</span><br /><br />'
        )
        .pauseFor(1000)
        .start();
    });
  }

  toggleRulesModal = () => {
    this.setState(prevState => ({
      modal: !prevState.modal
    }));
  };

  render() {
    const { points, x, y } = this.props;

    return (
      <div className="gui">
        <div
          className={cx("gui--info-modal", {
            "gui--info-modal--active": this.state.modal
          })}
        >
          <div className="gui--info-modal-content">
            <button
              type="button"
              className="gui--info-modal-close"
              onClick={this.toggleRulesModal}
            >
              <FontAwesomeIcon icon={faTimes} />
            </button>
            Space Crap instructor! <br />
            <br />
            To control your space vacuumer you need to adopt your hand for 🚀
            mouse! <br />
            <br />
            Using your rocket mouse you have to move around and collect as many
            as possible space items! <br />
            <br />
            Good luck with your journey and help our solar sistem!
            <br />
            <br />
            Regards, <br />
            Space crap crew!
            <br />
          </div>
        </div>
        <div className="gui--console"></div>
        <div className="gui--coordinates">
          {x} | {y}
        </div>
        <div className="gui--points">Points: {points}</div>

        <div className="gui--info">
          <button type="button" onClick={this.toggleRulesModal}>
            <FontAwesomeIcon icon={faInfoCircle} />
          </button>
        </div>
      </div>
    );
  }
}

export default GUI;
