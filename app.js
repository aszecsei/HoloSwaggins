/**
 * Copyright 2017, Google, Inc.
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START app]
'use strict';

// [START setup]
const express = require('express');
const bodyParser = require('body-parser');
const Buffer = require('safe-buffer').Buffer;
const vision = require('@google-cloud/vision');
const {Translate} = require('@google-cloud/translate');
const projectId = 'argon-key-218614';
const WordPOS = require('wordpos'),
 wordpos = new WordPOS();
// Instantiates a client
const translate = new Translate();

const client = new vision.ImageAnnotatorClient();
const app = express();
var multer = require('multer');
var upload = multer();
app.set('case sensitive routing', true);
app.use(bodyParser.urlencoded({     // to support URL-encoded bodies
  extended: true
})); 
app.use(bodyParser.json());
app.use(upload.array()); 
app.use(express.static('public'));

// [END setup]

app.post('/transcribe/models', (req, res) => {
  const request = {
    image: {
      content: req.body.image.replace(/^data:image\/png;base64,/, ''),
    },
    feature: {
      languageHints: ['en-t-i0-handwrit'],
    },
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation.text;
      return wordpos.getNouns(fullTextAnnotation)
    })
    .then(results =>{
      res.status(200).send(JSON.stringify(results) );
    })
    .catch(err => {
      res.status(500).send(err)
    });
});


app.post('/transcribe', (req, res) => {
  const request = {
    image: {
      content: req.body.image.replace(/^data:image\/png;base64,/, ''),
    },
    feature: {
      languageHints: ['en-t-i0-handwrit'],
    },
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      console.log(fullTextAnnotation.text)
      res.status(200).send(`${fullTextAnnotation.text}` );
    })
    .catch(err => {
      res.status(500).send(err)
    });
});

app.post('/transcribe/es', (req, res) => {
  const request = {
    image: {
      content: req.body.image,
    },
    feature: {
      languageHints: ['en-t-i0-handwrit'],
    },
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      return translate
        .translate(`${fullTextAnnotation.text}`, 'es')
    })
    .then(results => {
      const translation = results[0];
      console.log(translation)
      res.status(200).send(`${translation}` )
    })
    .catch(err => {
      res.status(500).send(err)
    });
});
app.post('/transcribe/de', (req, res) => {
  const request = {
    image: {
      content: req.body.image,
    },
    feature: {
      languageHints: ['en-t-i0-handwrit'],
    },
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      return translate
        .translate(`${fullTextAnnotation.text}`, 'de')
    })
    .then(results => {
      const translation = results[0];
      console.log(translation)
      res.status(200).send(`${translation}` )
    })
    .catch(err => {
      res.status(500).send(err)
    });
});
app.post('/transcribe/en', (req, res) => {
  const request = {
    image: {
      content: req.body.image,
    },
    feature: {
      languageHints: ['en-t-i0-handwrit'],
    },
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      return translate
        .translate(`${fullTextAnnotation.text}`, 'en')
    })
    .then(results => {
      const translation = results[0];
      console.log(translation)
      res.status(200).send(`${translation}` )
    })
    .catch(err => {
      res.status(500).send(err)
    });
});
app.post('/transcribe/fr', (req, res) => {
  const request = {
    image: {
      content: req.body.image,
    },
    feature: {
      languageHints: ['en-t-i0-handwrit'],
    },
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      return translate
        .translate(`${fullTextAnnotation.text}`, 'fr')
    })
    .then(results => {
      const translation = results[0];
      console.log(translation)
      res.status(200).send(`${translation}` )
    })
    .catch(err => {
      res.status(500).send(err)
    });
});




if (module === require.main) {
  // [START listen]
  const PORT = process.env.PORT || 8080;
  app.listen(PORT, () => {
    console.log(`App listening on port ${PORT}`);
    console.log('Press Ctrl+C to quit.');
  });
  // [END listen]
}
// [END app]

module.exports = app;
