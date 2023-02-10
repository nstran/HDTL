import {palette} from './palette';

/**
 * Roles for colors.  Prefer using these over the palette.  It makes it easier
 * to change things.
 *
 * The only roles we need to place in here are the ones that span through the app.
 *
 * If you have a specific use-case, like a spinner color.  It makes more sense to
 * put that in the <Spinner /> component.
 */
export const color = {
    /**
     * The palette is available to use, but prefer using the name.
     */
    palette,
    /**
     * A helper for making something see-thru. Use sparingly as many layers of transparency
     * can cause older Android devices to slow down due to the excessive compositing required
     * by their under-powered GPUs.
     */
    transparent: 'rgba(0, 0, 0, 0)',
    /**
     * The screen background.
     */
    background: palette.white,
    /**
     * The main tinting color.
     */
    primary: '#2D2252',
    black: '#1d1d1d',
    white: '#ffffff',
    offWhite: '#e6e6e6',
    orange: '#f88111',
    orangeDarker: '#EB9918',
    lightGrey: '#939AA4',
    lighterGrey: '#CDD4DA',
    angry: '#dd3333',
    deepPurple: '#5D2555',
    danger: '#F0268A',
    tabbar: '#1A1334',
    green: '#2BB56C',
    blue: '#2C79BD',
    warning: '#FFC400',
    xanh_xam:'#182954',
    xanh_nhat:'#41B1DF',
    trang_nhat: "#F9F9F9",
    nau_nhat: '#F1F1F1',
    text_naunhat: '#797979',
    nau_nhat2: '#9F9F9F',
    xam : '#898A8D',
    blue_nhat: '#E1EEFB',
    trang_nhat_2: '#F5F5F5',
    /**
     * The main tinting color, but darker.
     */
    primaryDarker: palette.orangeDarker,
    /**
     * A subtle color used for borders and lines.
     */
    line: palette.offWhite,
    /**
     * The default color of text in many components.
     */
    text: palette.white,
    /**
     * Secondary information.
     */
    dim: palette.lightGrey,
    /**
     * Error messages and icons.
     */
    error: palette.angry,

    /**
     * Storybook background for Text stories, or any stories where
     * the text color is color.text, which is white by default, and does not show
     * in Stories against the default white background
     */
    storybookDarkBg: palette.black,

    /**
     * Storybook text color for stories that display Text components against the
     * white background
     */
    storybookTextColor: palette.black,
};
