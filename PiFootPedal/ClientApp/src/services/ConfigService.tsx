import PollConfig from "../models/response/PollConfig";
import BaseService from "./BaseService";

export default class ConfigService extends BaseService {
    baseUrl: string;

    constructor(isDebug: boolean) {
        super();
        let piUrl = "192.168.1.76";
        //piUrl = "localhost";
        this.baseUrl = isDebug ? `https://${piUrl}:8000/api/v1` : "/api/v1";//localhost
    }

    getConfig() {
        return this.get<PollConfig>(`${this.baseUrl}/config`);
    }

    saveConfig(config: PollConfig){
        return this.postData(`${this.baseUrl}/config`, config);
    }
}