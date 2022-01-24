import ButtonAction from "./ButtonAction";

export default class PollConfig {
    isVerbose!: boolean;
    buttonPins!: number[];
    buttonActions!: Record<number, ButtonAction>;
}