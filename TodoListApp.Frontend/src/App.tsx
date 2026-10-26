import { useState } from "react";
import LoginPanel from "./components/LoginPanel";
import "./App.css";

function App() {
  const [count, setCount] = useState(0);

  return (
    <>
      <LoginPanel onSave={() => alert("on save")} />
      <p>app. Count {count}</p>
    </>
  );
}

export default App;
