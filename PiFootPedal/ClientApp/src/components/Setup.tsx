import SetupService from "../services/SetupService";
import BaseComponent from "./BaseComponent";
import ButtonComponent from "./ButtonComponent";

type SetupProps = {
    isDebug: boolean;
};

type SetupState = {
    isStarted: boolean;
    buttonPins: number[];
};

export default class Setup extends BaseComponent<SetupProps, SetupState> {
    
    setupService: SetupService;
    setupUpdate: number | undefined

    constructor(props: SetupProps){
        super(props);
        this.setupService = new SetupService(props.isDebug);
        this.state = {
            isStarted: false,
            buttonPins: []
        }
    }
    
    async startSetup() {
        let success = await this.setupService.startSetup();
        if(success){
            this.setupUpdate = window.setInterval(() => this.updatePins.call(this), 1000);
            this.setState({
                isStarted: success,
                buttonPins: []
            });
        }
    }

    async updatePins() {
        let setup = this;
        this.setupService.getButtons()
            .then(data => {
                setup.setState({
                    buttonPins: Array.from(data.buttonPins)
                });
                
            })
            .catch(err => {
                console.log(err);
                setup.setupUpdate && clearInterval(setup.setupUpdate);
                setup.setState({
                    isStarted: false
                });
            }
        );
    }

    async stopSetup(save: boolean) {
        console.log(this.setupService);
        let success = await this.setupService.stopSetup(save);
        if(success){
            this.setupUpdate && clearInterval(this.setupUpdate);
            this.setState({
                isStarted: !success
            });
        }
    }

    render(){
        return <>
            <h1>Setup</h1>
            {!this.state.isStarted ? 
                <>
                    <button className={`btn btn-sm btn-success`} onClick={() => this.startSetup()}>Start Setup</button><br/><br/>
                    <p>Press Start Setup to begin mapping the buttons on your foot pedal to pins on the raspberry pi.</p>
                </> :
                <>
                    <button className={`btn btn-sm btn-danger`} onClick={() => this.stopSetup(false)}>Cancel Setup</button><br/><br/>
                    <p>Press the buttons on your foot pedal in order from left to right.</p>
                    <h2>Buttons:</h2>
                    {this.state.buttonPins.length === 0 ?
                        <div>
                            No buttons found yet... waiting...
                        </div> :
                        <div>
                            <div>{this.state.buttonPins.map(btn => <ButtonComponent button={btn} isSelected={false} />)}</div><br/>
                            <button className={`btn btn-sm btn-success`} onClick={() => this.stopSetup(true)}>Save Setup</button>
                        </div>
                        
                    }
                </>
            }
        </>
    }
}