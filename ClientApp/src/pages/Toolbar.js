import React from 'react';
import { Button, ButtonGroup } from 'react-bootstrap';
import { BsFillPersonCheckFill, BsFillPersonDashFill, BsFillTrashFill } from 'react-icons/bs';

const Toolbar = ({ onSelectAll, onDeselectAll, onBlock, onUnblock, onDelete }) => (
    <ButtonGroup>
    <Button variant="primary" onClick={onSelectAll}>Select All</Button>
    <Button variant="secondary" onClick={onDeselectAll}>Deselect All</Button>
    <Button variant="danger" onClick={onBlock}><BsFillPersonDashFill /> Block</Button>
    <Button variant="success" onClick={onUnblock}><BsFillPersonCheckFill /> Unblock</Button>
    <Button variant="warning" onClick={onDelete}><BsFillTrashFill /> Delete</Button>
  </ButtonGroup>
);

export default Toolbar;
