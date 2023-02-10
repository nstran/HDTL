import { initializeApp } from 'firebase/app';
import {
  getAuth,
  onAuthStateChanged,
  createUserWithEmailAndPassword,
  sendEmailVerification } from 'firebase/auth';
import {
  Database,
  getDatabase,
  ref as firebaseDatabaseRef,
  set as firebaseSet,
  child,
  get,
  onValue,
  query, 
  orderByChild,
  equalTo,
  limitToFirst,
  limitToLast,
  push,
  update
} from 'firebase/database';

// Firebase config
const firebaseConfig = {
  apiKey: "AIzaSyACMDaqqLlMypmR3aQctCyVKhN94ehafOo",
  authDomain: "hello-world-34e33.firebaseapp.com",
  databaseURL: "https://hello-world-34e33.firebaseio.com",
  projectId: "hello-world-34e33",
  storageBucket: "hello-world-34e33.appspot.com",
  messagingSenderId: "822529047154",
  appId: "1:822529047154:android:1a9509198f5c3e8520e0eb",
};
// initialize firebase
const app = initializeApp(firebaseConfig);
const auth = getAuth();
const firebaseDatabase = getDatabase();

export {
  auth,
  createUserWithEmailAndPassword,
  firebaseDatabase,
  firebaseDatabaseRef,
  firebaseSet,
  child,
  get,
  onValue,
  query, 
  orderByChild,
  equalTo,
  Database,
  limitToFirst,
  limitToLast,
  push,
  update
}