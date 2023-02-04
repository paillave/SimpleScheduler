import React from 'react';
// import clsx from 'clsx';
import styles from './Presentation.module.css';

const PresentationImage = require('../../static/img/signalr-fast-scalable-gauge-bot-racecar.svg').default;

export default function Presentation() {
  return (
    <section className={styles.features}>
      <PresentationImage className={styles.featureSvg} />
      <div className={styles.paragraphDescription}>
        <h1>Scheduling</h1>
        <p>Scheduler.NET is a <b>scheduler for .NET</b> workers that heavily relies on <b>Dependency Injection</b>. Therefore, the scheduling setup is directly linked to the business layer. Any business layer can interact with the scheduler as well.</p>
      </div>
    </section>
  );
}
