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
"use strict";

// [START setup]
const express = require("express");
const bodyParser = require("body-parser");
const Buffer = require("safe-buffer").Buffer;
const vision = require("@google-cloud/vision");
const { Translate } = require("@google-cloud/translate");
const projectId = "argon-key-218614";
const WordPOS = require("wordpos"),
  wordpos = new WordPOS();
// Instantiates a client
const translate = new Translate();

const client = new vision.ImageAnnotatorClient();
const app = express();
var multer = require("multer");
var upload = multer();
app.set("case sensitive routing", true);
app.use(
  bodyParser.urlencoded({
    // to support URL-encoded bodies
    extended: true
  })
);
app.use(bodyParser.json());
app.use(upload.array());
app.use(express.static("public"));
const GoogleImages = require("google-images");

const imclient = new GoogleImages("ENGINE_ID", "API_KEY");

// [END setup]

app.post("/transcribe/model", (req, res) => {
  const request = {
    image: {
      content: req.body.image.replace(/^data:image\/png;base64,/, "")
    },
    feature: {
      languageHints: ["en-t-i0-handwrit"]
    }
  };
  nm;
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation.text;
      return wordpos.getNouns(fullTextAnnotation);
    })
    .then(results => {
      if (results.length > 3) {
        results = [
          results[0],
          results[results.length / 2],
          results[results.length]
        ];
        let map = results.map(test => {
          imclient.search(test).then(next => {
            return next ? next[0].url : null;
          });
        });

        res.status(200).send(JSON.stringify(map));
      }
      let map = results.map(test => {
        client.search(test).then(next => {
          return next ? next[0].url : null;
        });
      });
      res.status(200).send(JSON.stringify(map));
    })
    .catch(err => {
      res.status(500).send(err);
    });
});

app.post("/transcribe", (req, res) => {
  const request = {
    image: {
      content: req.body.image.replace(/^data:image\/png;base64,/, "")
    },
    feature: {
      languageHints: ["en-t-i0-handwrit"]
    }
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      console.log(fullTextAnnotation.text);
      fullTextAnnotation.text
        ? res.status(200).send(`${fullTextAnnotation.text}`)
        : res.status(200).send("No text");
    })
    .catch(err => {
      res.status(500).send(err);
    });
});

app.post("/transcribe/es", (req, res) => {
  const request = {
    image: {
      content: req.body.image
    },
    feature: {
      languageHints: ["en-t-i0-handwrit"]
    }
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      if (fullTextAnnotation.text) {
        return translate.translate(`${fullTextAnnotation.text}`, "es");
      } else {
        res.status(200).send("No text");
      }
    })
    .then(results => {
      const translation = results[0];
      console.log(translation);
      res.status(200).send(`${translation}`);
    })
    .catch(err => {
      res.status(500).send(err);
    });
});
app.post("/transcribe/de", (req, res) => {
  const request = {
    image: {
      content: req.body.image
    },
    feature: {
      languageHints: ["en-t-i0-handwrit"]
    }
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      if (fullTextAnnotation.text) {
        return translate.translate(`${fullTextAnnotation.text}`, "de");
      } else {
        res.status(200).send("No text");
      }
    })
    .then(results => {
      const translation = results[0];
      console.log(translation);
      res.status(200).send(`${translation}`);
    })
    .catch(err => {
      res.status(500).send(err);
    });
});
app.post("/transcribe/en", (req, res) => {
  const request = {
    image: {
      content: req.body.image
    },
    feature: {
      languageHints: ["en-t-i0-handwrit"]
    }
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      if (fullTextAnnotation.text) {
        return translate.translate(`${fullTextAnnotation.text}`, "en");
      } else {
        res.status(200).send("No text");
      }
    })
    .then(results => {
      const translation = results[0];
      console.log(translation);
      res.status(200).send(`${translation}`);
    })
    .catch(err => {
      res.status(500).send(err);
    });
});
app.post("/transcribe/fr", (req, res) => {
  const request = {
    image: {
      content: req.body.image
    },
    feature: {
      languageHints: ["en-t-i0-handwrit"]
    }
  };
  client
    .documentTextDetection(request)
    .then(results => {
      const fullTextAnnotation = results[0].fullTextAnnotation;
      if (fullTextAnnotation.text) {
        return translate.translate(`${fullTextAnnotation.text}`, "fr");
      } else {
        res.status(200).send("No text");
      }
    })
    .then(results => {
      const translation = results[0];
      console.log(translation);
      res.status(200).send(`${translation}`);
    })
    .catch(err => {
      res.status(500).send(err);
    });
});

if (module === require.main) {
  // [START listen]
  const PORT = process.env.PORT || 8080;
  app.listen(PORT, () => {
    console.log(`App listening on port ${PORT}`);
    console.log("Press Ctrl+C to quit.");
  });
  // [END listen]
}
// [END app]

module.exports = app;
