import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import registerServiceWorker from './registerServiceWorker';
import PowerFxDemoPage from './PowerFxDemoPage';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

const script = document.createElement('script');
script.type = 'text/javascript';
script.src = 'http://localhost:7080/_framework/blazor.webassembly.js';
script.setAttribute("autostart", "false");
script.crossOrigin = "anonymous";
script.onload = async () => {
  // @ts-ignore
  // eslint-disable-next-line no-undef
  await Blazor.start({
    loadBootResource: function (type, name, defaultUri, integrity) {
      console.log(`Loading: '${type}', '${name}', '${defaultUri}', '${integrity}'`);
      switch (type) {
        case 'dotnetjs':
          return `${process.env.REACT_APP_PFX_WASM_HOST}/_framework/${name}`;
        default:
          return fetch(`${process.env.REACT_APP_PFX_WASM_HOST}/_framework/${name}`, {
            credentials: 'omit'
          });
      }
    }
  });

  // Create a callable JS function that calls through to the exposed C#.
  // window.sayHello = async function (name) {
  //   // eslint-disable-next-line no-undef
  //   return await DotNet.invokeMethodAsync(
  //     'PowerFxWasm',
  //     'SayHello',
  //     name,
  //   );
  // };

  ReactDOM.render(
    <BrowserRouter basename={baseUrl}>
      <PowerFxDemoPage />
    </BrowserRouter>,
    rootElement);
};
document.body.appendChild(script);



// registerServiceWorker();

