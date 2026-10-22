import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type OrderLineRecord = {
  id: string | number;
  description: string;
  quantity: number;
  unit?: string;
  unitPrice: number;
  discountRate?: number;
  vatRate?: number;
  totalPrice?: number;
  lineOrder?: number;
  orderId: string;
  productId: string;
};

interface OrderLineEditProps {
  record: OrderLineRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const OrderLineEdit: React.FC<OrderLineEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<OrderLineRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<OrderLineRecord>) =>
      dataProvider.update('OrderLine', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'OrderLine', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Edit OrderLine</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Description *" value={String(form.description ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('description', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Quantity *" value={String(form.quantity ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('quantity', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Unit" value={String(form.unit ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('unit', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Unit Price *" value={String(form.unitPrice ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('unitPrice', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Discount Rate" value={String(form.discountRate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('discountRate', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Vat Rate" value={String(form.vatRate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('vatRate', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Total Price" value={String(form.totalPrice ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('totalPrice', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Line Order" value={String(form.lineOrder ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('lineOrder', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Sipariş *" resource="Order" value={String(form.orderId ?? '')} onChange={(v) => setField('orderId', v)} displayField="orderNo" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Ürün *" resource="Product" value={String(form.productId ?? '')} onChange={(v) => setField('productId', v)} displayField="name" />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <TkButton label="Cancel" variant="secondary" onTkClick={onClose} />
            <TkButton label={mutation.isPending ? 'Saving…' : 'Save Changes'} variant="primary" mode="submit" disabled={mutation.isPending} />
          </div>
        </form>
      </div>
    </div>
  );
};
