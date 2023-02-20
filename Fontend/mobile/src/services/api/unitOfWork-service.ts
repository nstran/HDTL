import { ApisauceInstance, create } from "apisauce"
import { ApiConfig, DEFAULT_API_CONFIG } from "./api-config"
import { UserApi } from "./user-api"
import { Storage } from '../storage/index'
/**
 * Manages all requests to the API.
 */
export class UnitOfWorkService {
  private _userApi: UserApi | undefined
  private apisauce: ApisauceInstance
  private _storage: Storage | undefined;
  /**
   * Configurable options.
   */
  private config: ApiConfig = DEFAULT_API_CONFIG

  /**
   * Creates the api.
   *
   * @param config The configuration to use.
   */
  constructor() {
    this.apisauce = create({
      baseURL: this.config.url,
      timeout: this.config.timeout,
      headers: {
        'Accept': 'application/json',
        // 'Origin': 'http://103.138.113.52:56',
        'Origin': 'http://203.171.21.35',
        'Content-Type': 'application/json',
      },
    })
  }

  /**
   * Sets up the API.  This will be called during the bootup
   * sequence and will happen before the first React component
   * is mounted.
   *
   * Be as quick as possible in here.
   */
  get user(): UserApi {
    // if (this._userApi == null) {
      return (this._userApi = new UserApi(this.apisauce))
    // }
    // return this._userApi
  }
  get storage(): Storage {
    // if (this._storage == null) {
      return (this._storage = new Storage())
    // }
    // return this._storage
  }
}
