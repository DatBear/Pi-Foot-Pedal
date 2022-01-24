import BaseComponent from "./BaseComponent";

import './ButtonComponent.css';

type ButtonProps = {
    button: number;
    isSelected: boolean;
    onClick?: React.MouseEventHandler<HTMLButtonElement> | undefined;
};

type ButtonState = {

}

export default class ButtonComponent extends BaseComponent<ButtonProps, ButtonState>{

    render(){
        return <button key={this.props.button} className={`button-selector${this.props.isSelected ? ' selected ' : ''}`} onClick={this.props.onClick}>
            <i className="fas fa-hockey-puck fa-3x"></i><div>{this.props.button}</div>
        </button>
    }
}