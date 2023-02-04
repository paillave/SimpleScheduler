import React from 'react';
import clsx from 'clsx';
import Layout from '@theme/Layout';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import styles from './index.module.css';
import HomepageFeatures from '../components/HomepageFeatures';
import HomepageExamples from '../components/HomepageExamples';
import QuickStarts from '../components/QuickStart';
import Videos from '../components/Videos';
import Presentation from '../components/Presentation';
// import Highlight, { defaultProps } from "prism-react-renderer";
// require(`prismjs/components/prism-csharp`); // eslint-disable-line
// import theme from "prism-react-renderer/themes/dracula";

const Bot = require('../../static/img/bot-racecar.svg').default;
const EtlNet = require('../../static/img/full-black-logo.svg').default
// const streamImage = require('../../static/img/SmallStreams.jpg').url;
const SponsorImage = require('../../static/img/Sponsor.svg').default;
function HomepageHeader() {
  const { siteConfig } = useDocusaurusContext();
  return (
    <header className={clsx('hero hero--primary', styles.heroBanner)}>
      <div className="container">
        <div className={clsx(styles.sponsor)}><a href='https://www.fundprocess.lu' ><SponsorImage /></a></div>
        <Bot className={clsx(styles.kayakBot)} />
        <div className={clsx(styles.mainLogoBackground)}>
          <EtlNet />
        </div>
        {/* <h1 className="hero__title">{siteConfig.title}</h1> */}
        <p className="hero__subtitle">{siteConfig.tagline}</p>
      </div>
    </header>
  );
}

export default function Home() {
  const { siteConfig } = useDocusaurusContext();
  return (
    <Layout
      title={`Home`}
      description="Fully featured ETL int .NET for .NET working with the same principle than SSIS">
      <HomepageHeader />
      <main>
        <Presentation />
        <HomepageFeatures />
        <Videos/>
        <QuickStarts />
        <HomepageExamples />
      </main>
    </Layout>
  );
}
