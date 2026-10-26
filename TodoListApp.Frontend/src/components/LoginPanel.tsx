import { useEffect, useState } from "react";
import { getCookie, setCookie } from "../api/loginHelper";

type Props = {
  onSave: () => void;
};
export default function LoginPanel({ onSave }: Props) {
  const [inputToken, setInputToken] = useState("");

  useEffect(() => {
    function load() {
      setInputToken(getCookie("todo_token"));
      console.log("login_panel_load");
    }
    load();
  }, []);

  return (
    <div className="token-controls">
      <div>
        <label className="form-control form-input" htmlFor="user-token">
          Токен
        </label>
        <input
          className="form-control form-input"
          id="user-token"
          type="password"
          value={inputToken}
          onChange={(e) => setInputToken(e.target.value)}
          placeholder="Enter token"
        />
        <button
          className="btn btn-success"
          onClick={() => {
            setCookie("todo_token", inputToken);
            console.log(`token: ${inputToken}`);
            onSave();
          }}
        >
          Save Token
        </button>
        <button
          className="btn btn-danger"
          onClick={() => {
            setCookie("todo_token", "");
            onSave();
          }}
        >
          Clear Token
        </button>
      </div>
    </div>
  );
}
