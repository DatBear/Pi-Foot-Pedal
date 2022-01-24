import { Component } from 'react';

export default class BaseComponent<TProps, TState> extends Component<TProps, TState> {
  handleInputChange(property : string) {
    return (e: any) => {
        //console.log('prev state:', this.state);
        //console.log('property: ', property, 'value: ',e.target.value.toString());
        this.setState({
            [property]: e.target.value
        } as any);
    };
  }

  enumKeys<O extends object, K extends keyof O = keyof O>(obj: O): K[] {
    return Object.keys(obj).filter(k => Number.isNaN(+k)) as K[];
  }
}