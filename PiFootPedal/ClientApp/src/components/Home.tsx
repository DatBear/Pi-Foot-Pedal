import React from 'react';
import ButtonActionTypes from '../models/ButtonActionTypes';
import Keys from '../models/Keys';
import ModifierKeys from '../models/ModifierKeys';
import ButtonAction from '../models/response/ButtonAction';
import KeyDescription from '../models/response/KeyDescription';
import PollConfig from '../models/response/PollConfig';
import ConfigService from '../services/ConfigService';
import BaseComponent from './BaseComponent';
import ButtonComponent from './ButtonComponent';

import './Home.css';

type HomeProps = {

};

type HomeState = {
  config: PollConfig | null;
  buttonSelected: number | null;
  addingKey: KeyDescription | null;
  preSleepMs: string;
  postSleepMs: string;
};

export class Home extends BaseComponent<HomeProps, HomeState> {
  static displayName = Home.name;
  configService: ConfigService;

  constructor(props: HomeProps){
    super(props);
    this.state = {
      config: null,
      buttonSelected: null,
      addingKey: null,
      preSleepMs: '',
      postSleepMs: ''
    };
    let isDebug = window.location.href.indexOf('localhost') > -1;
    this.configService = new ConfigService(isDebug);

    this.getConfig();
  }

  async getConfig(){
    let config = await this.configService.getConfig();
    config.buttonPins.filter(x => config.buttonActions[x] == null).forEach(x => config.buttonActions[x] = new ButtonAction());
    this.setState({config: config});
  }

  async saveConfig(){
    if(this.state.config == null) return;
    this.configService.saveConfig(this.state.config).then(x => {
      alert('Configuration saved.');//todo change
    }).catch(ex => {
      alert('Error saving config.');//todo change
    });
  }

  selectButton(button: number){
    this.setState({
      buttonSelected: button
    });
  }

  setButtonType(button: number | null, type: ButtonActionTypes){
    let config = this.state.config;
    if(config === null || button === null || config.buttonActions[button] === null) return;
    config.buttonActions[button].type = type;
    this.setState({
      config: config
    });
  }

  addKey(){
    this.setState({
      addingKey: new KeyDescription()
    });
  }

  setAddingKey(key: Keys){
    if(this.state.addingKey == null) return;
    this.state.addingKey.key = key;
    this.setState({ addingKey: this.state.addingKey });
  }

  setAddingModifier(mod: ModifierKeys){
    if(this.state.addingKey == null) return;
    this.state.addingKey.modifier = mod;
    this.setState({ addingKey: this.state.addingKey })
  }

  selectedButtonAction(){
    let config = this.state.config;
    return config?.buttonActions[this.state.buttonSelected ?? 0];    
  }

  confirmAddKey(){
    let config = this.state.config;
    let button = config?.buttonActions[this.state.buttonSelected ?? 0];
    if(config == null || button == null || this.state.addingKey == null) return;
    this.state.addingKey.preSleepMs = Number(this.state.preSleepMs) > 0 ? Number(this.state.preSleepMs) : null;
    this.state.addingKey.postSleepMs = Number(this.state.postSleepMs) > 0 ? Number(this.state.postSleepMs) : null;
    button.keys.push(this.state.addingKey);
    this.setState({config: config, addingKey: null});

  }

  removeKey(idx: number){
    console.log('delete', idx);
    let config = this.state.config;
    let button = config?.buttonActions[this.state.buttonSelected ?? 0];
    if(config == null || button == null) return;
    button.keys.splice(idx, 1);
    this.setState({
      config: config
    });
  }

  componentDidMount(){
    this.getConfig();
  }

  renderKey(key: KeyDescription, idx: number) {
    let deleteButton = true && <button className='btn btn-sm btn-link remove-button' onClick={() => this.removeKey(idx)}><i className="fas fa-trash fa-xs"></i></button>;
    let preSleep = key.preSleepMs != null ? <span className='pre-sleep'><i className="far fa-clock"></i>{key.preSleepMs}ms</span> : '';
    let postSleep = key.postSleepMs != null ? <span className='post-sleep'><i className="far fa-clock"></i>{key.postSleepMs}ms</span> : '';
    let mod = key.modifier ? ModifierKeys[key.modifier] + " + " : '';
    return <div key={idx} className="key">{deleteButton} {preSleep} <i className="fas fa-key fa-xs"></i>{mod}{Keys[key.key]} {postSleep}</div>
  }

  renderButton(btn: number){
    return <button key={btn} className={`button-selector${this.state.buttonSelected === btn ? ' selected ' : ''}`} onClick={() => this.selectButton(btn)}>
      <i className="fas fa-hockey-puck fa-3x"></i><div>{btn}</div>
    </button>
  }

  render () {
    let isSelected = this.state.config != null && this.state.buttonSelected != null
    let selectedAction = isSelected ? this.state.config?.buttonActions[this.state.buttonSelected ?? 0] ?? new ButtonAction() : null;

    return (
      <div>
        <h1>Configure Buttons</h1>
        <p>Select a button below to change how it is mapped.</p>
        <div className="button-selectors">
          {this.state.config && this.state.config.buttonPins.map(btn => <ButtonComponent button={btn} isSelected={this.state.buttonSelected === btn} onClick={() => this.selectButton(btn)} />)}
        </div>
        
        {isSelected && selectedAction && <>
          <h3>Button {this.state.buttonSelected} Configuration</h3>
          <div className="type">
            Type:
            {
              this.enumKeys(ButtonActionTypes).map((bat, idx) => {
                return <button key={idx} className={`btn-sm btn-link${selectedAction?.type === ButtonActionTypes[bat] ? ' selected': ''}`} onClick={() => this.setButtonType(this.state.buttonSelected, ButtonActionTypes[bat])}>{bat}</button>
              })
            }
          </div>
          <div className="keys">
            Keys:
            <button className='add-button btn-link' onClick={() => this.addKey()}><i className="fas fa-plus"></i></button><br/>
            {(this.selectedButtonAction()?.keys?.length ?? 0) === 0 && !this.state.addingKey && <span className='text-center'><br/><em>No keys configured yet.<br/> Add a key by clicking the <i className="fas fa-plus"></i>.</em></span>}
            {selectedAction.keys.map((key, idx) => this.renderKey(key, idx))}
            {this.state.addingKey && <div className="adding-key">
              <b>New Key:</b><br/>
              Modifier: {this.enumKeys(ModifierKeys).map((mod, idx) => {
                return <button key={idx} className={`btn-sm btn-link${(this.state.addingKey?.modifier ?? ModifierKeys.None) === ModifierKeys[mod] ? ' selected' : ''}`} onClick={() => this.setAddingModifier(ModifierKeys[mod])}>{mod}</button>
              })}<br/>
              Key: {this.enumKeys(Keys).map((key, idx) => {
                return <button key={idx} className={`btn-sm btn-link${this.state.addingKey?.key === Keys[key] ? ' selected' : ''}`} onClick={() => this.setAddingKey(Keys[key])}>{key}</button>
              })}<br/>
              <i className="far fa-clock" /> Before: <input value={isNaN(Number(this.state.preSleepMs)) ? '' : Number(this.state.preSleepMs)} onChange={this.handleInputChange('preSleepMs')} />ms<br/>
              <i className="far fa-clock" /> After: <input value={isNaN(Number(this.state.postSleepMs)) ? '' : Number(this.state.postSleepMs)} onChange={this.handleInputChange('postSleepMs')} />ms<br/>
              {this.state.addingKey.key && <button className='btn btn-sm btn-success' onClick={() => this.confirmAddKey()}>Add Key</button>}
            </div>}
          </div><br/><br/>
          <button className='btn btn-sm btn-success' onClick={() => this.saveConfig()}>Save all changes</button>
        </>}
      </div>
    );
  }
}
