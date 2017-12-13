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
      console.log('[ServiceWorker] Caching app shell');
      return cache.addAll(filesToCache);
    })
  );
});

self.addEventListener('fetch', function (event) {
  event.respondWith(
    caches.match(event.request)
      .then(function (response) {
        console.log("[ServiceWorker] Cache hit - return response for " + event.request.url);
        if (response) {
          return response;
        }

        // Fallback to server.
        var fetchRequest = event.request.clone();
        return fetch(fetchRequest).then(
          function (response) {
            if (!response || response.status !== 200 || response.type !== 'basic') {
              console.log("[ServiceWorker] Fetch got an invalid response for " + event.request.url);
              return response;
            }

            var responseToCache = response.clone();
            caches.open(cacheName)
              .then(function (cache) {
                console.log("[ServiceWorker] Caching response for " + event.request.url);
                cache.put(event.request, responseToCache);
              });

            return response;
          });
      })
  );
});
