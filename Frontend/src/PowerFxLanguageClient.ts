/*!
 * Copyright (C) Microsoft Corporation. All rights reserved.
 */

import { sendDataAsync } from './Helper';

export class PowerFxLanguageClient {
    public constructor(private _onDataReceived: (data: string) => void) {
    }

    public async sendAsync(data: string) {
        console.log('[LSP Client] Send: ' + data);

        try {
            const result = await DotNet.invokeMethodAsync<string>("PowerFxWasm", "LspAsync", data)
            // if (!result.ok) {
            //     return;
            // }

            // const response = await result.text();
            if (result) {
                const responseArray = JSON.parse(result);
                responseArray.forEach((item: string) => {
                    console.log('[LSP Client] Receive: ' + item);
                    this._onDataReceived(item);
                })
            }
        } catch (err) {
            console.log(err);
        }
    }
}
