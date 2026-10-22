import React from 'react';
import { TkCard, TkAlert, TkButton, TkTabs, TkTabsItem, TkDialog, TkTreeView, TkOrgChart, TkTimeline, TkStepper, TkStep } from '@takeoff-ui/react';

// Neutral UI wrappers — dashboards/shared components import these so the same
// generated JSX renders on any UI framework. This file is the Takeoff impl.

export const UiCard: React.FC<{ header?: string; bodyStyle?: React.CSSProperties; children?: React.ReactNode }> = ({ header, bodyStyle, children }) => (
  <TkCard {...(header ? { header } : {})}>
    <div slot="content" style={bodyStyle}>{children}</div>
  </TkCard>
);

export const UiAlert: React.FC<{ variant?: string; message?: string; header?: string }> = ({ variant = 'info', message, header }) => (
  <TkAlert variant={variant as never} message={message} {...(header ? { header } : {})} />
);

export const UiButton: React.FC<{ label?: string; variant?: string; onClick?: () => void; disabled?: boolean }> = ({ label, variant = 'primary', onClick, disabled }) => (
  <TkButton label={label} variant={variant as never} onTkClick={onClick} disabled={disabled} />
);

export const UiTabs: React.FC<{ children?: React.ReactNode }> = ({ children }) => (
  <TkTabs>{children}</TkTabs>
);

export const UiTabsItem: React.FC<{ label: string; children?: React.ReactNode }> = ({ label, children }) => (
  <TkTabsItem label={label}>{children}</TkTabsItem>
);

export const UiDialog: React.FC<{ visible: boolean; header?: string; onClose: () => void; footer?: React.ReactNode; children?: React.ReactNode }> = ({ visible, header, onClose, footer, children }) => (
  <TkDialog visible={visible} header={header} onTkClose={onClose}>
    <div slot="content" style={{ padding: '16px 0', fontSize: 14, color: '#333' }}>{children}</div>
    <div slot="footer" style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', padding: '12px 16px', width: '100%', boxSizing: 'border-box' }}>{footer}</div>
  </TkDialog>
);

export type UiTreeItem = { key: string; label: string; children?: UiTreeItem[] };
export type UiOrgNode = { id: string; parentId: string | null; name: string; title: string };

export const UiTreeView: React.FC<{ items: UiTreeItem[] }> = ({ items }) => (
  <TkTreeView items={items} expandAll />
);


const escapeHtml = (s: string) => s.replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' } as Record<string, string>)[c]);
const orgNodeHtml = (d: { width: number; height: number; data: { name: string; title: string } }) =>
  '<div style="box-sizing:border-box;width:' + d.width + 'px;height:' + d.height + 'px;border:1px solid #E1E4EA;border-radius:8px;background:#fff;' +
  'padding:8px 12px;display:flex;flex-direction:column;justify-content:center;align-items:center;text-align:center;font-family:var(--family-body, sans-serif)">' +
  '<div style="font-size:14px;font-weight:600;color:#222530;line-height:18px;overflow-wrap:anywhere">' + escapeHtml(d.data.name ?? '') + '</div>' +
  (d.data.title ? '<div style="font-size:12px;color:#6B7280;line-height:16px;margin-top:4px;overflow-wrap:anywhere">' + escapeHtml(d.data.title) + '</div>' : '') +
  '</div>';

type OrgChartApi = {
  nodeWidth(f: () => number): OrgChartApi;
  nodeHeight(f: () => number): OrgChartApi;
  nodeContent(f: (d: { width: number; height: number; data: UiOrgNode }) => string): OrgChartApi;
  render(): OrgChartApi;
};
type OrgChartElement = HTMLElement & { componentOnReady?: () => Promise<unknown>; getOrgChart?: () => Promise<OrgChartApi | undefined> };

// d3-org-chart tek kok ister — rowsToOrgNodes gerekirse ortak kok ekler.
// Kutu genisligi tk-org-chart'ta sabit (160px); getOrgChart() ile d3 ornegine erisip ayarlanir.
export const UiOrgChart: React.FC<{ nodes: UiOrgNode[]; nodeWidth?: number }> = ({ nodes, nodeWidth = 220 }) => {
  const ref = React.useRef<OrgChartElement>(null);
  React.useEffect(() => {
    let cancelled = false;
    (async () => {
      await customElements.whenDefined('tk-org-chart');
      await ref.current?.componentOnReady?.();
      const chart = await ref.current?.getOrgChart?.();
      if (!cancelled && chart) chart.nodeWidth(() => nodeWidth).nodeHeight(() => 76).nodeContent(orgNodeHtml).render();
    })();
    return () => { cancelled = true; };
  }, [nodes, nodeWidth]);
  return nodes.length === 0 ? <div style={{ color: '#999', fontSize: 13 }}>Kayıt yok</div> : (
    <div style={{ height: 480 }}><TkOrgChart ref={ref as never} data={nodes} collapsible /></div>
  );
};

export const UiTimeline: React.FC<{ items: Array<{ title: string; date?: string; description?: string }>; orientation?: 'vertical' | 'horizontal' }> = ({ items, orientation = 'vertical' }) => (
  <TkTimeline items={items} orientation={orientation} />
);

export const UiStepper: React.FC<{ steps: string[]; active?: number }> = ({ steps, active = 0 }) => (
  <TkStepper active={active} stepMode="number" showCompleteState>
    {steps.map((s, i) => <TkStep key={i} header={s} />)}
  </TkStepper>
);
