> WIP, details TBD

# Run locally

1. Build `PowerFxWasm` project
1. Navigate to outputs, for example ``
1. Install `http-server` (via npm, `npm -g i http-server`)
1. Run `http-server --port 7080 --cors`
1. Navigate to frontend
1. Create file called `.env.local`
1. Put the local web server's URL as variable: `REACT_APP_PFX_WASM_HOST="http://localhost:7080"`
1. Navigate to frontend, run `npm run start`
1. Open app at http://localhost:3000