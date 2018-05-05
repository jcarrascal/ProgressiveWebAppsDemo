# Progressive Web Applications Demo
A Progressive Web Applications should also work even when the device is offline. This can be achieved by using a Service Worker. A Service Worker is a JavaScript file that runs on the background, doesn't have access to the DOM but can intercept and cache requests for when the device is offline.

We add a sw.js file for the Service Worker and define which URLs should be cached as an array:
```javascript
var cacheName = 'expenses-app-v1';
var filesToCache = [
  "/",
  "/lib/bootstrap/dist/css/bootstrap.theme.css",
  "/css/site.css",
  "/lib/jquery/dist/jquery.js",
  "/lib/bootstrap/dist/js/bootstrap.js",
  "/js/site.js",
  "/lib/bootstrap/dist/fonts/glyphicons-halflings-regular.woff2"
];

self.addEventListener('install', function (e) {
  console.log('[ServiceWorker] Install');
  e.waitUntil(
    caches.open(cacheName).then(function (cache) {
      return cache.addAll(filesToCache);
    })
  );
});
```

To intercept the calls we handle the "fetch" event:
```javascript
self.addEventListener('fetch', function (event) {
  event.respondWith(
    caches.match(event.request)
      .then(function (response) {
        console.log("[ServiceWorker] Cache hit - return response for " + event.request.url);
        if (response) {
          return response;
        }
        return fetch(event.request);
      }
    )
  );
});
```

Finally, we have to register the Service Worker from the site. So, on the site.js file we add the following code:
```javascript
if ('serviceWorker' in navigator) {
    window.addEventListener('load', function () {
        navigator.serviceWorker.register('/sw.js').then(function (registration) {
            console.log('ServiceWorker registration successful with scope: ', registration.scope);
        }, function (err) {
            console.log('ServiceWorker registration failed: ', err);
        });
    });
}
```

## Handling POST
To be able to store expenses and deposits, even when we are offline, we have to store the requests and, when we are connected again, send them to the server.

For simplicity, I added some code to site.js that handles the error callback of the jQuery's ajax() method and stores the movements on an array. The next successful request will also trigger the processing of the stored requests.

A better solution would be to handle the requests from the Service Worker and store the requests on the Indexed DB, but for demo purposes this solution is much more complicated.
