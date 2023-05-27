import React from "react";
import styles from "./ModalWindow.module.scss";
import { blackCoverState } from "../../state/BlackCoverState";
import { observer } from "mobx-react-lite";

const ModalWindow = observer(() => {
  const modelWindowEntity = blackCoverState.modalWindow;

  return (
    <div className={styles.window}>
      <div className={styles.textContainer}>
        <p className={styles.text}>
          {
            modelWindowEntity?.text
          }
        </p>
      </div>
      <div className={styles.buttonsContainer}>
        <button className={styles.okButton} onClick={modelWindowEntity?.okAction}>Ok</button>
        <button className={styles.cancelButton} onClick={modelWindowEntity?.cancelAction}>Cancel</button>
      </div>
    </div>
  );
});

export default ModalWindow;