export function setCookie(name: string, value: string, days = 365) {
  if (typeof document === "undefined") return;
  const expires = new Date(
    Date.now() + days * (1000 * 60 * 60 * 24),
  ).toUTCString();
  document.cookie = `${name}=${encodeURIComponent(value)}; expires=${expires}; path=/`;
}
export function getCookie(name: string) {
  if (typeof document === "undefined") return "";
  return document.cookie.split("; ").reduce((r, v) => {
    const parts = v.split("=");
    return parts[0] === name ? decodeURIComponent(parts.slice(1).join("=")) : r;
  }, "");
}

export async function sha256(str: string): Promise<string> {
  const encoder = new TextEncoder();
  const bytes = encoder.encode(str); // UTF8.GetBytes

  const hashBuffer = await crypto.subtle.digest("SHA-256", bytes); // ComputeHash

  const hashArray = Array.from(new Uint8Array(hashBuffer));

  const hashHex = hashArray
    .map((b) => b.toString(16).padStart(2, "0")) // "x2"
    .join("");

  return hashHex;
}
