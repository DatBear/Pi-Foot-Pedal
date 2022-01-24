import PollSetup from "../models/response/PollSetup";
import BaseService from "./BaseService";

export default class SetupService extends BaseService {
    baseUrl: string;

    constructor(isDebug: boolean){
        super();
        let piUrl = "192.168.1.73";
        this.baseUrl = isDebug ? `https://${piUrl}:8000/api/v1` : "/api/v1";//localhost
    }

    startSetup() {
        return this.post<boolean>(`${this.baseUrl}/setup/start`);
    }

    stopSetup(save: boolean) {
        return this.post<boolean>(`${this.baseUrl}/setup/stop?save=${save ? 'true' : 'false'}`);
    }

    getButtons() {
        return this.get<PollSetup>(`${this.baseUrl}/setup`);
    }
}