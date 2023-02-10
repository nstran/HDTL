importScripts("https://www.gstatic.com/firebasejs/9.1.3/firebase-app-compat.js");
importScripts("https://www.gstatic.com/firebasejs/9.1.3/firebase-messaging-compat.js");

firebase.initializeApp({
    apiKey: "AIzaSyD0PPSJWsnyDiqR8q0Gjovbu-v6yNi3L6I",
    authDomain: "hello-world-34e33.firebaseapp.com",
    databaseURL: "https://hello-world-34e33.firebaseio.com",
    projectId: "hello-world-34e33",
    storageBucket: "hello-world-34e33.appspot.com",
    messagingSenderId: "822529047154",
    appId: "1:822529047154:web:44a68df1d51118ac20e0eb",
    measurementId: "G-H758XN301S",
  });
  
  const messaging = firebase.messaging();