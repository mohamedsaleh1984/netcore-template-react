import { getAccessToken } from "src/Utils";
export class BaseClient {
  protected transformOptions(options: any): Promise<any> {
  
      options.headers = {
        ...options.headers,
        Authorization: `Bearer ${getAccessToken()}`,
      };
    
    return Promise.resolve(options);
  }
}