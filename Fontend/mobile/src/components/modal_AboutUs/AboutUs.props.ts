import { StyleProp, TextStyle, ViewStyle } from "react-native"
// import { IconTypes } from "../icon/icons"
// import { TxKeyPath } from "../../i18n"

export interface AboutProps {
  /**
   * Main header, e.g. POWERED BY IGNITE
   */
  headerTx?: TxKeyPath
  model_about_us?:boolean

  /**
   * header non-i18n
   */
  headerText?: string

  /**
   * Icon that should appear on the left
   */
  // leftIcon?: IconTypes

  /**
   * What happens when you press the left icon
   */
  onLeftPress?(): void
  closeModal?():void

  /**
   * Icon that should appear on the right
   */
  // rightIcon?: IconTypes

  /**
   * What happens when you press the right icon
   */
  onRightPress?(): void

  /**
   * Container style overrides.
   */
  style?: StyleProp<ViewStyle>

  /**
   * Title style overrides.
   */
  titleStyle?: StyleProp<TextStyle>
}
