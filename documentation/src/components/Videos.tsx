import React from 'react';
import clsx from 'clsx';
import styles from './HomepageFeatures.module.css';

interface IVideoList {
  url: string;
  title: string;
}

const videoList: IVideoList[] = [];

export default function HomepageFeatures() {
  if (!videoList?.length) return null;
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          <div className={clsx('col col--3')} />
          {videoList.map((props, idx) => <div key={idx} className={clsx('col col--6')}>
            <section className={styles.features}>
              <iframe width="560" height="315" src={props.url} title={props.title} frameBorder={0} allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowFullScreen></iframe>
            </section>
          </div>)}
          <div className={clsx('col col--3')} />
        </div>
      </div>
    </section>
  );
}
