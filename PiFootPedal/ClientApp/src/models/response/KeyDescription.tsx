import Keys from "../Keys";
import ModifierKeys from "../ModifierKeys";

export default class KeyDescription {
    modifier!: ModifierKeys;
    key!: Keys;
    preSleepMs!: number | null;
    postSleepMs!: number | null;
}