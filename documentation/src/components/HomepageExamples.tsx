import React from 'react';
import clsx from 'clsx';
import styles from './HomepageExamples.module.css';
import "prismjs"; // eslint-disable-line
import { WithLineNumbers } from './WithLineNumbers';
require(`prismjs/components/prism-csharp`); // eslint-disable-line
// https://emojipedia.org/

const features: IExample[] = [];

interface IExample {
  title: string;
  sourceCode: string;
  description: JSX.Element;
}

function Example({ sourceCode, title, description }) {
  return (<div className={clsx('col col--10 col--offset-1')}>
    <div className='card margin--md shadow--tl'>
      <div className="card__header">
        <h3>{title} 🎶</h3>
      </div>
      <div className="card__body">
        <p>{description}</p>
        <WithLineNumbers sourceCode={sourceCode.trim()} />
      </div>
    </div>
  </div>
  );
}

export default function HomepageExamples() {
  if (!features?.length) return null;
  return (
    <section className={styles.features}>
      <div className="container">
        {/* <h1>Examples</h1> */}
        <div className="row">
          {features.map((props, idx) => (
            <Example key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
