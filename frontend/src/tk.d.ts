import React from 'react';

type TkProps = Record<string, unknown> & {
  children?: React.ReactNode;
  style?: React.CSSProperties;
  className?: string;
};

// React 19 tiplerinde global JSX namespace yok — React.JSX'e eklenir (18.3'te de gecerli)
declare module 'react' {
  namespace JSX {
    interface IntrinsicElements {
      'tk-card': TkProps;
      'tk-alert': TkProps;
      'tk-button': TkProps;
      'tk-tabs': TkProps;
      'tk-tabs-item': TkProps;
    }
  }
}

export {};
