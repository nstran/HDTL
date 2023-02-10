import { Component } from '@angular/core';
import * as firebase from 'firebase';

const firebaseConfig = {
  apiKey: "AIzaSyD0PPSJWsnyDiqR8q0Gjovbu-v6yNi3L6I",
  authDomain: "hello-world-34e33.firebaseapp.com",
  databaseURL: "https://hello-world-34e33.firebaseio.com",
  projectId: "hello-world-34e33",
  storageBucket: "hello-world-34e33.appspot.com",
  messagingSenderId: "822529047154",
  appId: "1:822529047154:web:44a68df1d51118ac20e0eb",
  measurementId: "G-H758XN301S"
};

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';

  constructor() {
    firebase.initializeApp(firebaseConfig);
  }
}
